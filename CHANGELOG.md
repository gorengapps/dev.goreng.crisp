# Changelog

All notable changes to this project will be documented in this file.

## [0.3.0] - 2025-12-23
### Fixed
- `ToAwaitable` hanging indefinitely if the tween was killed or interrupted without completing.

### Changed
- `Crisp.ReportError` now logs exceptions to Unity Console when no `OnError` listener is subscribed, preventing silent failures.

## [0.2.0] - 2025-12-23
### Changed
- `OnComplete` in fluent API has been renamed to `SetOnComplete` to better align with the setter naming convention.
- Documentation updated to reflect the API change.

## [0.1.0] - 2025-12-05
### Added
- Initial release of Goreng Crisp.
- Zero-allocation tweening engine.
- `IRunLoop` integration.
- Async/Await support.
