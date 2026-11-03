export interface ICategory {
  id: number;
  name: string;
  img: IImage;
  isActive?: boolean;
}

export interface IImage {
  src: string;
  alt: string;
}
