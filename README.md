# Monofoxe.Spriter

This is a heavily modified version of [SpriterDotNet](https://github.com/loodakrawa/SpriterDotNet).

The original was so bad I had to spend several weeks fixing it. 
The code is still way too overcomplicated and badly designed, but this turd can only be saved by a complete rewrite by someone who's not loodakrawa. 

But be warned, this is what I use for my games internally, and it's not as polished and/or shiny as my other stuff. 

Changes so far:

## Added

- Added Monofoxe implementation.
- Added an ability to manipulate bones and sprites at runtime.
- Z tilting support.

## Removed

- Removed default Monogame implementation.
- Removed Unity implementation.
- Removed frame caching. It did not play well with runtime bone manipulation and time slowdown.

## Changed

- Did a complete codestyle overhaul. The code is much more readable now.
- Greatly reduced code complexity. Removed unnecessary interfaces and hygely simplified initial setup.
- Did a bunch of renamings and class merges to get rid of nothing-classes.

## Fixed

- Fixed completely broken animation transitions. [Fix](https://github.com/loodakrawa/SpriterDotNet/issues/99)
- Fixed an issue where some interpolations were calculated incorrecty in edge cases.
