import { ResolveFn } from '@angular/router';
import { inject } from '@angular/core';
import { BrandService } from '../Services/brand.service';
import { Observable } from 'rxjs';
import { IBrand } from '../Interfaces/i-brand';

export const brandResolver: ResolveFn<Observable<IBrand[]>> = () => {
  return inject(BrandService).getBrands();
};
