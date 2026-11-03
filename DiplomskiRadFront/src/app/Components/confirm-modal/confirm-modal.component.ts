import { Component } from '@angular/core';
import {
  ConfirmService,
  IConfirmRequest,
} from '../../Services/confirm.service';

@Component({
  selector: 'app-confirm-modal',
  templateUrl: './confirm-modal.component.html',
  styleUrls: ['./confirm-modal.component.scss'],
})
export class ConfirmModalComponent {
  request: IConfirmRequest | null = null;

  constructor(private confirmService: ConfirmService) {
    this.confirmService.request$.subscribe(
      (request) => (this.request = request),
    );
  }

  confirm(): void {
    this.confirmService.respond(true);
  }

  cancel(): void {
    this.confirmService.respond(false);
  }
}
