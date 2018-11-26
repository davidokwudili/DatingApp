// for the returned header
export interface Pagination {
  currentPage: number;
  itemsPerPage: number;
  totalItems: number;
  totalPages: number;
}

// store a generic of type T for any class
export class PaginatedResult<T> {
  result: T;
  pagination: Pagination;
}
