import { ResolveFn } from '@angular/router';
import { inject } from '@angular/core';
import { GenderService } from '../Services/gender.service';
import { Observable } from 'rxjs';
import { IGender } from '../Interfaces/i-gender';

export const genderResolver: ResolveFn<Observable<IGender[]>> = () => {
  return inject(GenderService).getGender();
};
