export function LoadingSpinner() {
  return (
    <div className="space-y-6 py-2">
      <div className="space-y-2">
        <div className="h-7 w-40 rounded-lg bg-muted animate-pulse" />
        <div className="h-4 w-56 rounded bg-muted animate-pulse" />
      </div>
      <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-4">
        {Array.from({ length: 8 }).map((_, i) => (
          <div
            key={i}
            className="bg-card border border-border rounded-xl p-5 space-y-3"
            style={{ animationDelay: `${i * 60}ms` }}
          >
            <div className="flex justify-between">
              <div className="h-3.5 w-16 rounded bg-muted animate-pulse" />
              <div className="h-3.5 w-20 rounded bg-muted animate-pulse" />
            </div>
            <div className="h-4 w-36 rounded bg-muted animate-pulse" />
            <div className="h-3.5 w-28 rounded bg-muted animate-pulse" />
            <div className="h-5 w-24 rounded bg-muted animate-pulse" />
            <div className="h-9 w-full rounded-lg bg-muted animate-pulse mt-1" />
          </div>
        ))}
      </div>
    </div>
  );
}
