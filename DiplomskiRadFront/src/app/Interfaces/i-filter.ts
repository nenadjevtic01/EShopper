export interface IFilter {
  sizes: number[];
  brands: number[];
  categories: number[];
  genders: number[];
  keyword: string;
  sortOption: number;
  [key: string]: number[] | string | number;
}
