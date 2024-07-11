import { expect, test } from "vitest";

test("Automatic semicolon addition", () => {
    function noProblem() {
        return {
            run: "hello",
        };
    }

    function ohDear() {
        // Ignore the plethora of warnings thrown up here!
        return;
        {
            run: "hello";
        }
    }

    var myNoProblem = noProblem();
    var myOhDear = ohDear();

    expect(myNoProblem && myNoProblem.run).not.equal(myOhDear && myOhDear.run);
});

test("Scoping", () => {
    var scopeTest = {
        foo: "bar",

        run: function () {
            var self = this;
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
    expect(typeof NaN).toBe("number");
    expect(NaN === NaN).toBeFalsy();
});

test("Closures", () => {
    var myScope = {};

    (function (scope) {
        // This member variable *is* used below 😀
        var _private = "private";
        scope.canSee = true;
    })(myScope);

    expect(myScope._private).toBeUndefined();
    expect(myScope.canSee).toBeTruthy();
});

test("Comparing null", () => {
    var bar = null;
    expect(typeof bar).toBe("object");
    expect(bar !== null && typeof bar === "object").toBeFalsy();
});

test("Floating-point precision", () => {
    expect(0.1 + 0.2).not.toBe(0.3);
});

test("Function arguments", () => {
    function sumNamedArgs() {
        return arguments[0] + arguments[1];
    }

    function sumNoNamedArgs(x, y) {
        return x + y;
    }

    expect(sumNamedArgs(1, 2)).toBe(sumNoNamedArgs(1, 2));
});

test("Function apply and call", () => {
    function sum(x, y) {
        return x + y;
    }

    const apply = sum.apply(undefined, [1, 2]);
    const call = sum.call(undefined, 1, 2)
    expect(apply).toEqual(call);
});

test("Coercion", () => {
    expect(1 + "2" + "2").toBe("122");
    expect(1 + +"2" + "2").toBe("32");
    expect(1 + -"1" + "2").toBe("02");
    expect(+"1" + "1" + "2").toBe("112");
    expect("A" - "B" + "2").toBe("NaN2");
    expect("A" - "B" + 2).not.toBe("NaN");
});
