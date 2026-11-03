import { IImage } from './i-category';

export interface IBrand {
  id: number;
  name: string;
  img: IImage;
  isActive?: boolean;
}
