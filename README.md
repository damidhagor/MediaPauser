# MediaPauser

This .Net MAUI App implements a simple sleep timer for pausing media playback after a certain amount of time.

It is only built for Android and uses the ``AudioManager`` to send a pause media key press.

## Changelog

### 1.0.0
- Initial Release

### 1.0.1
- Fixed a bug where the updating notification would alert the user.

### 1.1.0
- Added a function to prolong the running timer by 5 minutes.
- Increased the start timer UI size.