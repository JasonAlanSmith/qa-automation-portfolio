// Environment-driven settings — never hard-code URLs or credentials.
export const config = {
  apiBaseUrl: __ENV.MOPS_API_BASE_URL || 'http://localhost:5009/api/v1',
  email: __ENV.MOPS_TEST_EMAIL || 'test@test.org',
  password: __ENV.MOPS_TEST_PASSWORD || 'test',
};
