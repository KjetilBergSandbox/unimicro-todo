export interface Task {
    id: number;
    title: string;
    completed: boolean;
    dueDate: string | null;
    tags: string[];
    createdAt: string;
    updatedAt: string | null;
}

export interface PagedResponse {
    items: Task[];
    total: number;
    limit: number;
    offset: number;
    next: string | null;
    prev: string | null;
}