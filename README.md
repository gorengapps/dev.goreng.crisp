# Goreng Crisp 🍟

A high-performance, zero-allocation tweening library for Unity 6+, designed for modern async/await workflows.

## Features

- **Zero Allocation**: No garbage generated during tween creation or execution.
- **Async/Await Support**: Built-in `Awaitable` and `Task` support with cancellation.
- **DisposeBag Integration**: Seamlessly handle cancellation with `dev.goreng.events.DisposeBag`.
- **IRunLoop Integrated**: Decoupled from `MonoBehaviour` updates for better architecture.
- **Fluent API**: Chainable methods for ease, loops, and callbacks.

## Installation

Add the package to your project manifest or install via local package.

Dependencies:
- `dev.goreng.loop`
- `dev.goreng.events`

## Setup

Initialize `Crisp` with an `IRunLoop` provider (usually in your Bootstrap or Entry Point):

```csharp
public class GameBootstrap : MonoBehaviour
{
    [SerializeField] private RunLoop _runLoop; // Implements IRunLoop

    private void Awake()
    {
        Crisp.Init(_runLoop);
    }
}
```

## Basic Usage

### Tweening Components

Crisp provides extension methods for common Unity components:

```csharp
// Move Transform
transform.Move(new Vector3(10, 0, 0), 1f);
transform.MoveX(10f, 1f);
transform.LocalMoveY(5f, 1f);

// Rotate Transform
transform.Rotate(new Vector3(0, 180, 0), 0.5f);
transform.RotateZ(90f, 0.5f); // Rotate around Z axis
transform.LocalRotateY(45f, 0.5f);

// Scale Transform
transform.Scale(Vector3.one * 2, 1f);
transform.ScaleX(2f, 0.5f); // Scale X only
transform.ScaleY(0.5f, 0.5f);

// Fade CanvasGroup or Image
canvasGroup.Fade(0f, 0.5f);
image.Fade(0.5f, 1f);

// Fill Image
image.FillAmount(1f, 2f);
```

### Relative Tweening

All Transform extensions support relative tweening via the optional `relative` parameter.

```csharp
// Move 5 units up from current position
transform.MoveY(5f, 1f, relative: true);

// Rotate 90 degrees more around Z axis
transform.RotateZ(90f, 1f, relative: true);

// Add 1 to current scale
transform.Scale(Vector3.one, 1f, relative: true);
```

### Fluent Chaining

Chain settings like Ease, Loops, and Callbacks:

```csharp
transform.Move(targetPos, 1f)
    .SetEase(EaseType.OutBack)
    .SetLoops(3, LoopType.Yoyo)
    .SetDelay(0.5f)
    .SetOnComplete(() => Debug.Log("Done!"));
```

## Advanced Usage

### Generic Value Tweening

Tween any float value using `Crisp.Value`. Useful for custom logic or UI text updates.

```csharp
// Tween from 0 to 100 over 1 second
Crisp.Value(0f, 100f, 1f, (val) => 
{
    // Update text only when integer value changes to avoid GC
    if (Mathf.FloorToInt(val) != lastVal)
    {
        label.SetText("{0:n0}", val);
        lastVal = (int)val;
    }
});
```

### Delays

Create efficient, allocation-free delays:

```csharp
// Fire callback after 2 seconds
Crisp.Delay(2f, () => Debug.Log("Delayed!"));
```

## Async & Cancellation

Crisp is built for `async/await`. You can await any tween.

### Using DisposeBag (Recommended)

Pass a `DisposeBag` to `ToAwaitable()` or `ToTask()` to automatically handle cancellation when the bag is disposed (e.g., when a View unloads).

```csharp
public async Awaitable RunAnimation()
{
    // Automatically cancels if _disposeBag is disposed
    await transform.Move(targetPos, 1f).ToAwaitable(_disposeBag);
    
    // Works for delays too
    await Crisp.Delay(0.5f).ToAwaitable(_disposeBag);
}
```

### Parallel Execution

Use `.ToTask(_disposeBag)` to run multiple tweens in parallel using `Task.WhenAll`.

```csharp
var t1 = image1.Fade(1f, 1f).ToTask(_disposeBag);
var t2 = image2.Fade(1f, 1f).ToTask(_disposeBag);

await Task.WhenAll(t1, t2);
```

### Manual Cancellation

You can also pass a `CancellationToken` directly:

```csharp
await transform.Move(end, 1f).ToAwaitable(cancellationToken);
```

## Error Handling

Crisp suppresses `OperationCanceledException` by default when using `ToAwaitable`. If a tween is cancelled, the await simply completes immediately, allowing your async method to exit gracefully.

Global errors (like exceptions in callbacks) are reported via `Crisp.OnError`:

```csharp
Crisp.OnError += (ex) => Debug.LogException(ex);
```

## Supported Easing

- Linear
- InQuad, OutQuad, InOutQuad
- InCubic, OutCubic, InOutCubic
- InQuart, OutQuart, InOutQuart
- InQuint, OutQuint, InOutQuint
- InSine, OutSine, InOutSine
- InExpo, OutExpo, InOutExpo
- InCirc, OutCirc, InOutCirc
- InElastic, OutElastic, InOutElastic
- InBack, OutBack, InOutBack
- InBounce, OutBounce, InOutBounce
- Custom (AnimationCurve or Func)
