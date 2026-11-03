import { ResolveFn } from '@angular/router';
import { inject } from '@angular/core';
import { SizeService } from '../Services/size.service';
import { Observable } from 'rxjs';
import { ISize } from '../Interfaces/i-size';

export const sizeResolver: ResolveFn<Observable<ISize[]>> = () => {
  return inject(SizeService).getSizes();
};
