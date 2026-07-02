# Architecture

Ancient Value Overlay is structured around small, replaceable services.

## Core pipeline

1. Capture a configured region.
2. Read reward rows.
3. Normalize and resolve item names.
4. Fetch prices and save the latest good snapshot.
5. Rank rows with `DecisionAdvisor`.
6. Present best row, total value, unknown rows, and source health.

## Milestones

### Milestone 1

- WPF shell
- config loading and saving
- poe.ninja price source
- price cache
- decision advisor tests

### Milestone 2

- region picker
- live preview
- row diagnostics table
- named calibration profiles

### Milestone 3

- transparent presentation layer
- summary line
- best-row highlight
- confidence and unknown row display

### Milestone 4

- session stats
- screenshot/debug export
- modular reward panel support
