import { expect, test } from "vitest";

test("Automatic semicolon addition", () => {
    function noProblem() {
        return {
            run: "hello",
        };
    }

    function ohDear() {
        return;
        // biome-ignore lint: deliberate for testing
        run: "hello";
    }

    const myNoProblem = noProblem();
    const myOhDear = ohDear();

    expect(myNoProblem?.run).not.equal(myOhDear?.run);
});

test("Scoping", () => {
    const scopeTest = {
        foo: "bar",

        run: function () {
            const self = this;
            expect(this.foo).toEqual("bar");
            expect(self.foo).toEqual("bar");

            (function () {
                expect(this && this.foo).toBeUndefined();
                expect(self.foo).toBe("bar");
            })();
        },
    };

    scopeTest.run();
});

test("Funky NaN", () => {
    expect(typeof Number.NaN).toBe("number");
    // biome-ignore lint: deliberate for testing
    expect(Number.NaN === Number.NaN).toBeFalsy();
    expect(Number.isNaN(Number.NaN)).toBeTruthy();
});

test("Closures", () => {
    const myScope = {};

    ((scope) => {
        // We attempt to reference this variable below 😀
        const _private = "private";
        scope.canSee = true;
    })(myScope);

    expect(myScope._private).toBeUndefined();
    expect(myScope.canSee).toBeTruthy();
});

test("Comparing null", () => {
    const bar = null;
    expect(typeof bar).toBe("object");
    expect(bar !== null && typeof bar === "object").toBeFalsy();
});

test("Floating-point precision", () => {
    expect(0.1 + 0.2).not.toBe(0.3);
});

test("Function arguments", () => {
    function sumNamedArgs() {
        // biome-ignore lint: deliberate for testing
        return arguments[0] + arguments[1];
    }

    const sumNoNamedArgs = (x, y) => x + y;

    expect(sumNamedArgs(1, 2)).toBe(sumNoNamedArgs(1, 2));
});

test("Function apply and call", () => {
    function sum(x, y) {
        return x + y;
    }

    const apply = sum.apply(undefined, [1, 2]);
    const call = sum.call(undefined, 1, 2);
    expect(apply).toEqual(call);
});

test("Coercion", () => {
    // biome-ignore lint: deliberate for testing
    expect(1 + "2" + "2").toBe("122");
    // biome-ignore lint: deliberate for testing
    expect(1 + +"2" + "2").toBe("32");
    // biome-ignore lint: deliberate for testing
    expect(1 + -"1" + "2").toBe("02");
    // biome-ignore lint: deliberate for testing
    expect(+"1" + "1" + "2").toBe("112");
    // biome-ignore lint: deliberate for testing
    expect("A" - "B" + "2").toBe("NaN2");
    expect("A" - "B" + 2).not.toBe("NaN");
});
