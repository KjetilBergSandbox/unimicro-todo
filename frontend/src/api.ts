import axios from "axios";
import type { PagedResponse } from "./types";

const API_BASE = "http://localhost:5000/api";

export const getTasks = (limit = 5, offset = 0) =>
  axios
    .get<PagedResponse>(`${API_BASE}/task?limit=${limit}&offset=${offset}`)
    .then(res => res.data);
