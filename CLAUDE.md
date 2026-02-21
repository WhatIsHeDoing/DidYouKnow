# Guidelines

## Project overview

A multi-language educational repository. Each language directory demonstrates interesting or surprising language features through unit tests. Languages: C#, C++, JavaScript, Perl, Python, Rust.

## Directory structure

```text
csharp/       C# (.NET 10, xUnit)
cpp/          C++ (g++, clang-format)
javascript/   TypeScript (pnpm, Biome, Vitest)
perl/         Perl (Test::Class, Perl::Critic)
python/       Python (uv, ruff, unittest)
rust/         Rust (cargo, rustfmt, clippy)
```

## Running tests

Use `just` for everything — do not invoke language tools directly.

```sh
just install      # install all dependencies
just run          # run all tests
just lint         # lint all languages
just spellcheck   # spellcheck the repo
just go           # install + run + spellcheck

just javascript   # lint + test one language
just python
just cpp
just perl
just rust
just csharp

just javascript-lint  # lint one language only
just python-lint
just cpp-lint
just perl-lint
just rust-lint
just csharp-lint

just update       # update all dependencies interactively
just migrate      # run Biome migrations

just docker       # build and run Docker container locally
```

## Adding a new "did you know" fact

1. Add a test method to the relevant language's test file
2. Run `just spellcheck` to check any new words; add unknowns to `cspell.yml` under `ignoreWords` (technical tokens) or `words` (real English words)
3. Run `just <language>` to confirm the test passes

## Language tooling

Each language's `just` recipe runs format checking, linting, and tests in that order. All checks must pass.

| Language   | Format          | Lint              | Test          |
| ---------- | --------------- | ----------------- | ------------- |
| C#         | dotnet format   | —                 | dotnet test   |
| C++        | clang-format    | —                 | g++           |
| JavaScript | Biome           | Biome             | Vitest        |
| Perl       | —               | Perl::Critic      | Test::Class   |
| Python     | ruff format     | ruff check        | unittest      |
| Rust       | cargo fmt       | cargo clippy      | cargo test    |

## GitHub Actions

Always pin actions to a full commit SHA rather than a mutable tag or branch:

```yaml
# Good
uses: actions/setup-node@6044e13b5dc448c55e2357c09f80417699197238

# Bad
uses: actions/setup-node@v6
uses: actions/setup-node@main
```

To find the commit SHA for a tag:

```sh
gh api repos/actions/setup-node/git/ref/tags/v6.2.0 --jq '.object.sha'
```

The CI matrix runs on `ubuntu-24.04` and `macos-15` with `fail-fast: false`.

## Node version

Pinned in `.nvmrc`. All three environments (local, Docker, CI) read from this file:

- Local: `nvm use`
- Docker: `nvm install` (reads `.nvmrc` automatically)
- CI: `actions/setup-node` with `node-version-file: .nvmrc`

To change the Node version, update `.nvmrc` only.

## Spellcheck

Config is at `cspell.yml`. When adding new code that introduces unrecognised tokens:

- `ignoreWords` — technical tokens that are not real words (e.g. `rustup`, `cpanm`)
- `words` — real English words the dictionary doesn't know (e.g. `Allman`, `Xunit`)

## Line endings

All text files use LF, enforced by `.gitattributes`. Never commit CRLF.
