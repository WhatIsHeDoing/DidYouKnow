# Contributing

## Adding a fact

Add a new test to the relevant file in that language's directory, following the same patterns as the existing tests. Each test should:

- Have a doc comment explaining the feature or quirk
- Demonstrate the behaviour with a minimal, self-contained example
- Assert the result rather than just printing it

## Adding a language

1. Create a directory for the language (e.g. `go/`)
2. Add a test file with a handful of interesting facts
3. Add a recipe to the `justfile` to lint and run the tests, and include it in the `run` target
4. Add a step to both CI workflows: `.github/workflows/build.yml` and `.github/workflows/dependabot-approve.yml`
5. If the language has a package manager, add an install recipe to the `justfile` and include it in the `install` target, and add the ecosystem to `.github/dependabot.yml`
6. Add any words unknown to the spell checker to `cspell.yml`

## Running locally

Install tooling (macOS):

```sh
just setup
```

Install dependencies:

```sh
just install
```

Run all tests:

```sh
just run
```

Run a single language:

```sh
just rust
just python
just javascript
```

Update all dependencies:

```sh
just update
```
