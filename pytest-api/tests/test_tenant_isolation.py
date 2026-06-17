"""Multi-tenant isolation — a hallmark guarantee of the target app.

Demonstrated techniques:
- asserting a cross-cutting invariant (every row a caller sees is their tenant's)
- an honest, configuration-gated cross-tenant test that *skips* rather than fakes
  when a second tenant isn't available
"""
from __future__ import annotations

import pytest

pytestmark = pytest.mark.tenant


def test_listed_customers_all_belong_to_callers_tenant(api):
    my_tenant = api.me().json()["tenant"]
    assert my_tenant, "the authenticated identity must carry a tenant id"

    body = api.get("/customers", params={"pageSize": 100}).json()

    foreign = [
        c for c in body["items"] if c.get("tenantSysId") not in (my_tenant, None)
    ]
    assert not foreign, f"tenant filter leaked rows from other tenants: {foreign}"


def test_other_tenant_cannot_read_my_customer(
    customer_factory, second_tenant_client
):
    # `second_tenant_client` skips this test cleanly unless a second tenant is
    # configured (MOPS_TEST_EMAIL_2 / _PASSWORD_2) — see conftest.
    mine = customer_factory()

    # Tenant B requests tenant A's customer by id -> it must be invisible (404).
    response = second_tenant_client.get(f"/customers/{mine['sysId']}")
    assert response.status_code == 404
