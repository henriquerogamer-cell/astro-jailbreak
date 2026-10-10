# ASTRO Jailbreak

Host and package artwork for ASTRO Jailbreak. The launcher targets PS5 firmware 12.60 and opens the Relapse runtime from this repository's immutable jsDelivr commit URL.

## Artwork

| File | Dimensions |
|---|---:|
| `assets/astro/icon0.png` | 512 × 512 |
| `assets/astro/pic0.png` | 1920 × 1080 |
| `assets/astro/pic1.png` | 1920 × 1080 |
| `assets/astro/logo-astro.png` | 1514 × 1001, transparent |
| `assets/astro/background.png` | 1920 × 1080 |
| `assets/astro/splash.png` | 1280 × 720 |

## Package build

Pushing to `dev/astro-jailbreak` runs `.github/workflows/build-pkg.yml`. A successful run uploads `ASTRO-Jailbreak.pkg` as the `ASTRO-Jailbreak-pkg` workflow artifact. The workflow verifies its pinned launcher archive, extracts the reference package, updates the title/deeplink and artwork, then rebuilds and checks the output package.

The package is a debug fPKG build. The CI check confirms package structure and metadata; installation and launch on a PS5 still need to be tested on the target console.

See [THIRD_PARTY.md](THIRD_PARTY.md) for upstream source and build-tool pins.
