#!/bin/bash

# Exit immediately if a command exits with a non-zero status.
set -e

# Ensure the 'Reflections' section is filled in the README.md
if grep -q '``' README.md; then
  echo "Please replace the placeholders in the Reflections section."
  exit 1
fi

echo "README.md is complete!"
