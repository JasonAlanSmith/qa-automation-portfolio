"""Pagination, search, and boundary behaviour on a paged list endpoint.

Demonstrated techniques:
- pagination contract (page size honoured; envelope metadata coherent)
- server-side search filtering, including the empty-result case
- boundary values (pageSize = 1) via parameterization
- deterministic, self-cleaning setup via the factory — no reliance on whatever
  data happens to be in the database
"""
from __future__ import annotations

import uuid

import pytest
from jsonschema import validate

from src.factories import unique_name
from src.schemas import PAGED_RESULT_SCHEMA

pytestmark = pytest.mark.regression


@pytest.mark.smoke
def test_list_honours_requested_page_size(api):
    response = api.get("/customers", params={"page": 1, "pageSize": 2})

    assert response.status_code == 200
    body = response.json()
    validate(instance=body, schema=PAGED_RESULT_SCHEMA)
    assert body["page"] == 1
    assert body["pageSize"] == 2
    assert len(body["items"]) <= 2


@pytest.mark.parametrize("page_size", [1, 2, 5])
def test_page_never_exceeds_requested_size(api, page_size):
    body = api.get("/customers", params={"pageSize": page_size}).json()
    assert len(body["items"]) <= page_size


def test_search_filters_to_matching_customers(api, customer_factory):
    # A unique token shared by a known set we create — fully deterministic, so the
    # assertion doesn't depend on pre-existing data.
    token = unique_name("needle")
    customer_factory(name=f"{token}-alpha")
    customer_factory(name=f"{token}-bravo")

    body = api.get("/customers", params={"search": token, "pageSize": 50}).json()

    names = [c["name"] for c in body["items"]]
    assert len(names) == 2
    assert all(token in name for name in names)


def test_search_with_no_match_returns_empty_page(api):
    body = api.get("/customers", params={"search": uuid.uuid4().hex}).json()

    assert body["items"] == []
    assert body["totalCount"] == 0


def test_pages_cover_the_set_without_overlap(api, customer_factory):
    # Create three customers sharing a token, then page through them two-at-a-time
    # (scoped by search) and assert full, non-overlapping coverage.
    token = unique_name("paging")
    created = {customer_factory(name=f"{token}-{i}")["sysId"] for i in range(3)}

    page1 = api.get("/customers", params={"search": token, "page": 1, "pageSize": 2}).json()
    page2 = api.get("/customers", params={"search": token, "page": 2, "pageSize": 2}).json()

    assert page1["totalCount"] == 3
    ids_page1 = {c["sysId"] for c in page1["items"]}
    ids_page2 = {c["sysId"] for c in page2["items"]}

    assert len(ids_page1) == 2 and len(ids_page2) == 1
    assert ids_page1.isdisjoint(ids_page2), "a row appeared on two pages"
    assert ids_page1 | ids_page2 == created, "pages did not cover exactly our set"
