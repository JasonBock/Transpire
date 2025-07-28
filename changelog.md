# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [1.1.0] - Not Yet Released

### Added
- Added an analyzer to detect `goto` usage (issue [#34](https://github.com/JasonBock/Transpire/issues/34))

### Fixed
- Fixed a cast issue with `DetectNonSeparatedDigitsCodeFix` (PR [#35](https://github.com/JasonBock/Transpire/pull/35))

## [1.0.0] - 2025-07-09

### Added
- Added `CreateVersion7()` as an option to create a new `Guid` (issue [#27](https://github.com/JasonBock/Transpire/issues/27))
- Added an analyzer and code fix to detect and remove `#region` and `#endregion` (issue [#2](https://github.com/JasonBock/Transpire/issues/2))
- Added an analyzer and code fix to add digit separators (issue [#22](https://github.com/JasonBock/Transpire/issues/22))
- Added an analyzer and code fix to recommend `IsNullOrWhiteSpace()` over `IsNullOrEmpty()` (issue [#28](https://github.com/JasonBock/Transpire/issues/28))
- Created a diagnostic for `System.Collections` non-generic usage (issue [#23](https://github.com/JasonBock/Transpire/issues/23))

## [0.5.1] - 2023-06-26

### Fixed
- Fixed an issue with `DispatchProxy` not being found (issue [#24](https://github.com/JasonBock/Transpire/issues/24))

## [0.5.0] - 2021-12-01

### Changed
- Upgraded project to target .NET 6 (issue [#13](https://github.com/JasonBock/Transpire/issues/13))

## [0.4.0] - 2021-12-01

### Added
- Added an analyzer to verify generic parameters to `DispatchProxy.Create()` (issue [#12](https://github.com/JasonBock/Transpire/issues/12))

## [0.3.0] - 2021-11-07

### Added
- Added an analyzer and code fix to recommend `TryParse()` over `Parse()` (issue [#9](https://github.com/JasonBock/Transpire/issues/9))
- Added an analyzer and code fix to remove `$` for strings that are not interpolated (issue [#10](https://github.com/JasonBock/Transpire/issues/10))

## [0.2.0] - 2021-07-01

### Added
- Added an analyzer and code fix to change `DateTime.Now` to `DateTime.UtcNow` (issue [#1](https://github.com/JasonBock/Transpire/issues/1))