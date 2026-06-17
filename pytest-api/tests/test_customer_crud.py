"""Customer CRUD lifecycle — the heart of a resource test suite.

Demonstrated techniques:
- full create -> read -> update -> delete lifecycle against a real resource
- self-cleaning via a factory fixture (no residue, even when a test fails)
- test independence (each test owns its data; execution order is irrelevant)
- schema/contract validation of the resource shape
- negative & boundary paths (unknown id, missing required field)
"""
from __future__ import annotations

import pytest
from jsonschema import validate

from src.factories import customer_payload, unique_name
from src.schemas import CUSTOMER_SCHEMA

pytestmark = pytest.mark.crud

# A well-formed GUID that will never exist — for not-found / negative paths.
# Deliberately NOT all-zeros: the API treats Guid.Empty as a *missing* required
# value (a 400 from the validation filter), which would mask the 404 we're after.
UNKNOWN_ID = "ffffffff-ffff-ffff-ffff-ffffffffffff"


@pytest.mark.smoke
def test_create_customer_returns_201_with_valid_body(customer_factory):
    name = unique_name("acme")
    customer = customer_factory(name=name)

    assert customer["sysId"], "created customer must carry a sysId"
    assert customer["name"] == name
    validate(instance=customer, schema=CUSTOMER_SCHEMA)


def test_created_customer_can_be_read_back(api, customer_factory):
    created = customer_factory()

    response = api.get(f"/customers/{created['sysId']}")
    assert response.status_code == 200
    fetched = response.json()
    assert fetched["sysId"] == created["sysId"]
    assert fetched["name"] == created["name"]
    validate(instance=fetched, schema=CUSTOMER_SCHEMA)


def test_update_customer_changes_persisted_name(api, customer_factory):
    created = customer_factory()
    new_name = unique_name("renamed")

    # Realistic round-trip: take the resource, modify it, save the whole thing back.
    put = api.put(f"/customers/{created['sysId']}", json={**created, "name": new_name})
    assert put.status_code == 204

    refetched = api.get(f"/customers/{created['sysId']}").json()
    assert refetched["name"] == new_name


def test_delete_customer_then_it_is_gone(api, customer_factory):
    sysid = customer_factory()["sysId"]

    assert api.delete(f"/customers/{sysid}").status_code == 204
    assert api.get(f"/customers/{sysid}").status_code == 404


def test_full_lifecycle_create_read_update_delete(api, customer_factory):
    # One test walking the entire lifecycle end to end.
    created = customer_factory(name=unique_name("lifecycle"))
    sysid = created["sysId"]

    assert api.get(f"/customers/{sysid}").status_code == 200

    api.put(f"/customers/{sysid}", json={**created, "isActive": False})
    assert api.get(f"/customers/{sysid}").json()["isActive"] is False

    assert api.delete(f"/customers/{sysid}").status_code == 204
    assert api.get(f"/customers/{sysid}").status_code == 404


# --- negative / boundary -------------------------------------------------

@pytest.mark.negative
def test_get_unknown_customer_returns_404(api):
    assert api.get(f"/customers/{UNKNOWN_ID}").status_code == 404


@pytest.mark.negative
def test_create_customer_rejects_missing_name(api, reference_ids):
    payload = customer_payload(reference_ids["kind"], reference_ids["theme"], name="")
    response = api.post("/customers", json=payload)
    assert response.status_code == 400, (
        f"expected 400 for an empty name, got {response.status_code}"
    )


@pytest.mark.negative
def test_update_unknown_customer_returns_404(api, reference_ids):
    payload = customer_payload(reference_ids["kind"], reference_ids["theme"])
    payload["sysId"] = UNKNOWN_ID
    assert api.put(f"/customers/{UNKNOWN_ID}", json=payload).status_code == 404


@pytest.mark.negative
def test_delete_unknown_customer_returns_404(api):
    assert api.delete(f"/customers/{UNKNOWN_ID}").status_code == 404
