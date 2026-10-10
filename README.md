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

The package workflow is manual. It needs a compatible PS5 web-launcher `eboot.bin` at
`assets/astro/eboot.bin`; the encrypted reference PKGs available to this build cannot provide
that module with the current open extraction path. Once the module is present, run
`.github/workflows/build-pkg.yml` from GitHub Actions. A successful run uploads
`ASTRO-Jailbreak.pkg` as the `ASTRO-Jailbreak-pkg` workflow artifact.

The generated file is a debug fPKG. CI checks the package structure and metadata; installation
and launch still need testing on the target PS5.

See [THIRD_PARTY.md](THIRD_PARTY.md) for upstream source and build-tool pins.
