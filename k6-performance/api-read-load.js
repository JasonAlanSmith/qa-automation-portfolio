import { check, group, sleep } from 'k6';
import http from 'k6/http';
import { Trend } from 'k6/metrics';

import { config } from './config.js';
import { authenticate } from './helpers.js';

// A custom metric so the read/list endpoints can be thresholded on their own,
// separately from the overall http_req_duration.
const listDuration = new Trend('list_endpoint_duration', true);

export const options = {
  // Two scenarios: a quick single-user smoke, then a ramping load profile.
  scenarios: {
    smoke: {
      executor: 'constant-vus',
      vus: 1,
      duration: '10s',
      tags: { scenario: 'smoke' },
    },
    load: {
      executor: 'ramping-vus',
      startTime: '10s',
      startVUs: 0,
      stages: [
        { duration: '15s', target: 10 }, // ramp up
        { duration: '20s', target: 10 }, // hold
        { duration: '10s', target: 0 }, // ramp down
      ],
      tags: { scenario: 'load' },
    },
  },
  // Pass/fail gates — a load test should assert SLOs, not just produce numbers.
  thresholds: {
    http_req_failed: ['rate<0.01'], // < 1% of requests error
    http_req_duration: ['p(95)<800'], // 95th percentile under 800ms
    list_endpoint_duration: ['p(95)<800'],
    checks: ['rate>0.99'], // > 99% of checks pass
  },
};

// Runs once; the returned value is handed to every VU iteration.
export function setup() {
  return { cookie: authenticate() };
}

export default function (data) {
  const params = { headers: { Cookie: data.cookie } };

  group('list customers (paged)', () => {
    const res = http.get(`${config.apiBaseUrl}/customers?page=1&pageSize=20`, {
      ...params,
      tags: { name: 'customers' },
    });
    listDuration.add(res.timings.duration);
    check(res, {
      'customers 200': (r) => r.status === 200,
      'customers paged envelope': (r) => r.json('items') !== undefined,
    });
  });

  group('list products (paged)', () => {
    const res = http.get(`${config.apiBaseUrl}/products?pageSize=20`, {
      ...params,
      tags: { name: 'products' },
    });
    listDuration.add(res.timings.duration);
    check(res, { 'products 200': (r) => r.status === 200 });
  });

  group('reference data', () => {
    const res = http.get(`${config.apiBaseUrl}/references/customer-kinds`, {
      ...params,
      tags: { name: 'references' },
    });
    check(res, { 'references 200': (r) => r.status === 200 });
  });

  sleep(1);
}
