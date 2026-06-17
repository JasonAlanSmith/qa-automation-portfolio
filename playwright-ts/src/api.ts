import { type APIRequestContext, request } from '@playwright/test';

import { config } from './config';

/**
 * A thin API helper for the *hybrid* E2E pattern: set up and tear down test data
 * through the API (fast, reliable), then assert behaviour through the UI. This is
 * its own authenticated context — independent of the browser session.
 */
export async function createApiContext(): Promise<APIRequestContext> {
  // Trailing slash + relative paths: Playwright resolves a leading-slash path
  // against the ORIGIN, which would drop the "/api/v1" base segment.
  const ctx = await request.newContext({
    baseURL: config.apiBaseUrl.replace(/\/+$/, '') + '/',
  });
  const res = await ctx.post('authentication/login', {
    data: { email: config.email, password: config.password },
  });
  if (!res.ok()) {
    throw new Error(`API login failed (${res.status()}): ${await res.text()}`);
  }
  return ctx;
}

async function resolveReferenceIds(
  ctx: APIRequestContext,
): Promise<{ kind: string; theme: string }> {
  const kinds = (await (await ctx.get('references/customer-kinds')).json()) as {
    sysId: string;
  }[];
  const themes = (await (
    await ctx.get('references/customer-themes')
  ).json()) as { sysId: string }[];
  return { kind: kinds[0].sysId, theme: themes[0].sysId };
}

export interface SeededCustomer {
  sysId: string;
  name: string;
}

export async function seedCustomer(
  ctx: APIRequestContext,
  name: string,
): Promise<SeededCustomer> {
  const { kind, theme } = await resolveReferenceIds(ctx);
  const res = await ctx.post('customers', {
    data: {
      name,
      isIndividual: false,
      isActive: true,
      kindSysId: kind,
      themeSysId: theme,
    },
  });
  if (res.status() !== 201) {
    throw new Error(`seedCustomer failed (${res.status()}): ${await res.text()}`);
  }
  return (await res.json()) as SeededCustomer;
}

export async function deleteCustomer(
  ctx: APIRequestContext,
  sysId: string,
): Promise<void> {
  await ctx.delete(`customers/${sysId}`);
}
