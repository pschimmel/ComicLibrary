# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [1.0.0] - 2023-12-29

### Added

- First initial release.

## [1.0.1] - 2024-01-01

### Fixed

- Fixed exception at startup.
- Fixed editing of years in table view.
- Fixed dirty flag not being reset when a library is saved.
- Fixed missing condition when copying comics.
- Fixed searching for comics.

### Changed

- Changed Libraries popup to use a wrap panel.
- Added changelog.
- Changed way how new libraries are created.

## [1.1.0] - 2024-01-20

### Added

- Added command for moving a comic to another library.
- Added option to copy properties of selected comic when adding a new one.
- Added option to automatically create a backup when saving files.
- Added command for renaming a series.
- Added report to print list of comics.
- Added fields to enter purchase price and estimated value.
- Added global currency.
- Added language as field.

### Fixed

- Fixed export of images.
- Fixed deleting of optional values (issue number, year) while editing.

## [1.1.1] - 2024-02-15

### Added

- Added option to print a list of issues.
- Added command in ribbon to close current library.

### Fixed

- Fixed editing of comic language in detail window.
- Fixed creating new libraries .

### Changed 

- Now using AvalonDock for comic tabs.