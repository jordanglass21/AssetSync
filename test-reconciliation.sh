#!/bin/bash

# chmod +x test-reconciliation.sh
# ./test-reconciliation.sh

LEGACY_URL="http://localhost:8080"
MODERN_URL="http://localhost:5289"
CHAOS_COUNT=200

run_audit() {
    curl -s "$MODERN_URL/api/reconciliation/run-audit" | python3 -c "import sys,json; print(json.load(sys.stdin)['totalDiscrepanciesFound'])"
}

echo "--- Baseline audit ---"
BASELINE=$(run_audit)
if [ "$BASELINE" -ne 0 ]; then
    echo "FAIL: expected 0 discrepancies, got $BASELINE"
    exit 1
fi
echo "PASS: 0 discrepancies"

echo "--- Injecting chaos ($CHAOS_COUNT rows) ---"
curl -s -X POST "$LEGACY_URL/api/legacy/chaos?count=$CHAOS_COUNT"
echo ""

echo "--- Post-chaos audit ---"
AFTER=$(run_audit)
if [ "$AFTER" -gt 0 ]; then
    echo "PASS: $AFTER discrepancies detected"
else
    echo "FAIL: chaos not detected"
    exit 1
fi

echo "--- Resetting legacy data ---"
curl -s -X POST "$LEGACY_URL/api/legacy/reset"
echo ""
echo "Done."