"use strict";

QUnit.test("Automatic semicolon addition", function(assert) {
    function noProblem() {
        return {
            run: "hello"
        };
    }

    function ohDear() {
        // Ignore the plethora of warnings thrown up here!
        // jshint ignore:start
        return
        {
            run: "hello"
        };
        // jshint ignore:end
    }

    var myNoProblem = noProblem();
    var myOhDear = ohDear();

    assert.notStrictEqual(
        myNoProblem && myNoProblem.run,
        myOhDear && myOhDear.run);
});

QUnit.test("Scoping", function(assert) {
    var scopeTest = {
        foo: "bar",
        
        run: function() {
            var self = this;
            assert.strictEqual(this.foo, "bar");
            assert.strictEqual(self.foo, "bar");

            (function() {
                assert.strictEqual(this && this.foo, undefined);
                assert.strictEqual(self.foo, "bar");
            }());
        }
    };

    scopeTest.run();
});

QUnit.test("Funky NaN", function(assert) {
    assert.strictEqual(typeof NaN, "number");
    assert.notStrictEqual(NaN, NaN);
});

QUnit.test("Closures", function(assert) {
    var myScope = {};

    (function(scope) {
        // This member variable *is* used below :)
        // jshint unused: false
        var _private = "private";
        scope.canSee = true;
    })(myScope);

    assert.strictEqual(myScope._private, undefined);
    assert.ok(myScope.canSee);
});

QUnit.test("Comparing null", function(assert) {
    var bar = null;
    assert.strictEqual(typeof bar, "object");
    assert.notOk((bar !== null) && (typeof bar === "object"));
});

QUnit.test("Floating-point precision", function(assert) {
    assert.notEqual(0.1 + 0.2, 0.3);
});

QUnit.test("Event ordering", function(assert) {
    var done = assert.async();

    var result = "";
    
    result += "1";
    setTimeout(function() { result += "2"; }, 100);
    setTimeout(function() { result += "3"; }, 0);
    result += "4";

    setTimeout(function() {
        assert.strictEqual(result, "1432", "3 after 1 and 4");
        done();
    }, 500);
});

QUnit.test("Function arguments", function(assert) {
    function sumNamedArgs() {
        return arguments[0] + arguments[1];
    }

    function sumNoNamedArgs(x, y) {
        return x + y;
    }

    assert.strictEqual(sumNamedArgs(1, 2), sumNoNamedArgs(1, 2));
});

QUnit.test("Function apply and call", function(assert) {
    function sum(x, y) {
        return x + y;
    }

    assert.strictEqual(
        sum.apply(undefined, [1, 2]),
        sum.call(undefined, 1, 2));
});

QUnit.test("Coercion", function(assert) {
    // Ignore confusing plusses warning.
    // jshint -W007  
    assert.strictEqual(1 +  "2" + "2", "122");
    assert.strictEqual(1 +  +"2" + "2", "32");
    assert.strictEqual(1 +  -"1" + "2", "02");
    assert.strictEqual(+"1" +  "1" + "2", "112");
    assert.strictEqual( "A" - "B" + "2", "NaN2");
    assert.notOk( "A" - "B" + 2, "NaN");
});

QUnit.test("Avoid stack overflow", function(assert) {
    // Recursive throwing.
    var list = [];

    for (var i = 0; i < 100000; ++i) {
        list[i] = i;
    }

    function recursiveWillStackOverflow() {
        // Ignore expected statement warning.
        // jshint -W030
        list.pop() && recursiveWillStackOverflow();
    }

    var startDate = new Date();
    
    assert.throws(recursiveWillStackOverflow, RangeError);
    
    var endThrowingTime = new Date() - startDate;

    // Recursive non-throwing.
    list = [];

    for (i = 0; i < 10000; ++i) {
        list[i] = i;
    }

    var done = assert.async();
    startDate = new Date();

    function recurseNoProblem() {
        if (new Date() - startDate < endThrowingTime && list.pop()) {
            return setTimeout(recurseNoProblem, 0);
        }

        assert.ok(1, "Still has not thrown");
        done();
    }
    
    recurseNoProblem();
});
