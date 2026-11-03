import { ResolveFn } from '@angular/router';
import { inject } from '@angular/core';
import { CategoryService } from '../Services/category.service';
import { Observable } from 'rxjs';
import { ICategory } from '../Interfaces/i-category';

export const categoryResolver: ResolveFn<Observable<ICategory[]>> = () => {
  return inject(CategoryService).getCategory();
};
