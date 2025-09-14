import type { Task, PagedResponse } from "./types";
import { useState, useEffect } from "react";
import { getTasks } from "./api";

function App() {
  const [tasks, setTasks] = useState<Task[]>([]);
  const [paged, setPaged] = useState<PagedResponse | null>(null);

  const fetchTasks = async (limit = 5, offset = 0) => {
    const data = await getTasks(limit, offset);
    setTasks(data.items);
    setPaged(data);
  };

  useEffect(() => {
    fetchTasks();
  }, []);

  const handleNext = () => {
    if (paged?.next) {
      const url = new URL(paged.next, window.location.origin);
      fetchTasks(
        Number(url.searchParams.get("limit")),
        Number(url.searchParams.get("offset"))
      );
    }
  };

  const handlePrev = () => {
    if (paged?.prev) {
      const url = new URL(paged.prev, window.location.origin);
      fetchTasks(
        Number(url.searchParams.get("limit")),
        Number(url.searchParams.get("offset"))
      );
    }
  };

  return (
    <div style={{ padding: "2rem" }}>
      <h1>Todo List</h1>
      <ul>
        {tasks.map((t) => (
          <li key={t.id}>
            <strong>{t.title}</strong> [{t.completed ? "✅" : "❌"}]{" "}
            {t.tags.join(", ")}
          </li>
        ))}
      </ul>
      <div style={{ marginTop: "1rem" }}>
        <button onClick={handlePrev} disabled={!paged?.prev}>
          Prev
        </button>
        <button onClick={handleNext} disabled={!paged?.next} style={{ marginLeft: "1rem" }}>
          Next
        </button>
      </div>
      <p>
        Showing {tasks.length} of {paged?.total ?? 0}
      </p>
    </div>
  );
}

export default App;