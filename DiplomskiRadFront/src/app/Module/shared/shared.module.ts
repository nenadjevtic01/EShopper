import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { PricePipe } from '../../Pipes/price.pipe';

@NgModule({
  declarations: [PricePipe],
  imports: [CommonModule],
  exports: [PricePipe],
})
export class SharedModule {}
