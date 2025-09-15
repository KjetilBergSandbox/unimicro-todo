import axios from "axios";
import type { PagedResponse } from "./types";

const API_BASE = import.meta.env.VITE_API_BASE;

export const getTasks = (limit = 5, offset = 0) =>
  axios
    .get<PagedResponse>(`${API_BASE}/task?limit=${limit}&offset=${offset}`)
    .then(res => res.data);

export const insertExampleTask = () =>
  axios.post(`${API_BASE}/task`, {
    title: "Example Task",
    completed: false,
    tags: ["example"],
  })
    .then(res => res.data);