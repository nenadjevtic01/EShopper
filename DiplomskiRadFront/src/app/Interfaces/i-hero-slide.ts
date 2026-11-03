export interface IHeroSlide {
  id: number;
  tagline: string;
  title: string;
  subtitle: string;
  buttonText: string;
  buttonLink: string;
  mediaType: 'image' | 'video';
  mediaSrc: string;
  sortOrder: number;
  isActive: boolean;
}
