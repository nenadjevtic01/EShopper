import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'price',
})
export class PricePipe implements PipeTransform {
  transform(price: number): string {
    if (price) {
      return `€${price.toFixed(2)}`;
    } else {
      return '';
    }
  }
}
