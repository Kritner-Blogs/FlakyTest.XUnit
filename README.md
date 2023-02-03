# FlakyTest.XUnit

[![Build and test](https://github.com/Kritner-Blogs/FlakyTest.XUnit/actions/workflows/ci.yml/badge.svg?branch=main)](https://github.com/Kritner-Blogs/FlakyTest.XUnit/actions/workflows/ci.yml)
[![Coverage Status](https://coveralls.io/repos/github/Kritner-Blogs/FlakyTest.XUnit/badge.svg?branch=main)](https://coveralls.io/github/Kritner-Blogs/FlakyTest.XUnit?branch=main)

![Latest NuGet Version](https://img.shields.io/nuget/v/FlakyTest.XUnit)
![License](https://img.shields.io/github/license/Kritner-Blogs/FlakyTest.XUnit)

## Overview

Do you have some flaky tests? Are you using XUnit? Do you want to retry your flaky tests a few times before considering them failures? Heck yeah brother! (You should probably actually fix the test so it's not flaky, but sometimes you just don't have time!)

If you still want an easy way to mark a test or two (but no more than that!) as flaky, then this might be the package for you.  What does marking a test as flaky do?  Well, it will attempt to run the test up to `n` times.  If any test run between `{0..n}` passes, then the test is considered a passing test.  If all tests fail between `{0..n}` then and only then is the test considered a failing test.  Marking a test as flaky *can help* alleviate transient pipeline failures, where you know that the test *mostly* passes, there's just something you haven't found in the test itself that makes it not pass all the time.

## Usage

### `FlakyFact` / `FlakyTheory`

It probably can't get much easier to consume this functionality. If you're already used to decorating your test methods with `[Fact]` you're almost there!

#### Example

```cs
using FlakyTest.XUnit.Attributes;

[FlakyFact("this test is heckin flaky my dude, follow up to fix this in JIRA-1234", 42)]
public async Task WhenDoingThing_ShouldHaveThisResult()
{
   // your test implementation which is sometimes flaky
}

[FlakyTheory("same idea as flaky fact, just using a theory. follow up to fix this in JIRA-1234", 42)]
[InlineData(true)]
[InlineData(false)]
public async Task WhenDoingThing_ShouldHaveThisResult(bool isDoots)
{
   // your test implementation which is sometimes flaky
}
```

The project _requires_ the first string parameter `flakyExplanation` to be not null/empty... cuz you really shouldn't be using this anyway. This can be used as a way to describe how/why the test is flaky, and perhaps leave a note and ticket for follow up to get the flakyness aspect of the test fixed.

The second parameter indicates how many times the test can "fail" before the test runner actually reports it as failure. The default value here (at the time of writing this) is 5 - meaning the test can fail 5 times prior to the runner reporting it as a failure. If the test _passes_ on the first, fourth, or any test in between, it is immediately marked as successful to the runner.

### `MaybeFixedFact` / `MaybeFixedTheory`

Did you have a test you were previously decorating as `Flaky`? Have you made updates to the code or test and want to check that it's no longer flaky? Well the attributes `MaybeFixedFact` and `MaybeFixedTheory` may be for you!

Use these attributes to signal to the test runner that the tests decorated as such should be run `n` times (and pass each of those times) to be considered  "successful".

If `n` is 10, the test will be run up to 10x. The runner will bail early if a failure is encountered up to `n`, once `n` is arrived at with _all_ passing tests, the test is considered an overall disposition of "passed". This `n` value is configurable, but is 10 by default (at the time of writing this documentation anyway).

#### Example

```cs
using FlakyTest.XUnit.Attributes;

[MaybeFixedFact(42)]
public async Task WhenDoingThing_ShouldHaveThisResult()
{
   // your test implementation which was sometimes flaky
   // checking now that it's no longer flaky by running
   // the test 42 times before (and they must all be individually passing) being considered a passed test
}

[FlakyTheory]
[InlineData(true)]
[InlineData(false)]
public async Task WhenDoingThing_ShouldHaveThisResult(bool isDoots)
{
   // your test implementation which was sometimes flaky
}
```

## Where these attributes won't work

`FlakyFact` and `FlakyTheory` are only meant to be used as stopgaps until a time in which the test issue can be diagnosed and fixed.  In some cases, depending on how exactly your test class, and test itself is set up, these attributes may not work for getting a "passing" flaky test.

How can this happen?  If you have a test that modifies state in such a way that the "starting state/data" for the test is different after a failed execution and attempt at rerun, it's likely you'll not be able to make successful use of the flaky attributes. 
