# FlakyTest.XUnit

Do you have some flaky tests? Are you using XUnit? Do you want to retry your flaky tests a few times before considering them failures? Heck yeah brother! (You should probably actually fix the test so it's not flaky, but sometimes you just don't have time!)

If you still want an easy way to mark a test or two (but no more than that!) as flaky, then this might be the package for you.

## Usage

It probably can't get much easier to consume this functionality.  If you're already used to decorating your test methods with `[Fact]` you're almost there!

```cs
using FlakyTest.XUnit.Attributes;

[FlakyFact("this test is heckin flaky my dude, follow up to fix this in JIRA-1234", 42)]
public async Task WhenDoingThing_ShouldHaveThisResult()
{
   // your test implementation which is sometimes flaky
}
```

The project *requires* the first string parameter `flakyExplanation` to be not null/empty... cuz you really shouldn't be using this anyway.  This can be used as a way to describe how/why the test is flaky, and perhaps leave a note and ticket for follow up to get the flakyness aspect of the test fixed.

The second parameter indicates how many times the test can "fail" before the test runner actually reports it as failure.  The default value here (at the time of writing this) is 5 - meaning the test can fail 5 times prior to the runner reporting it as a failure.  If the test *passes* on the first, fourth, or any test in between, it is immediately marked as successful to the runner.
