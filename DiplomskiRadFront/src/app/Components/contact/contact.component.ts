import { Component } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { ToastService } from '../../Services/toast.service';
import { ContactMessageService } from '../../Services/contact-message.service';

@Component({
  selector: 'app-contact',
  templateUrl: './contact.component.html',
})
export class ContactComponent {
  contactData: any = {};
  contactForm!: FormGroup;
  sending: boolean = false;

  constructor(
    private router: Router,
    private toastService: ToastService,
    private contactMessageService: ContactMessageService,
  ) {
    this.contactForm = new FormGroup<any>({
      name: new FormControl('', [
        Validators.required,
        Validators.minLength(3),
        Validators.maxLength(100),
      ]),
      email: new FormControl('', [Validators.required, Validators.email]),
      subject: new FormControl('', [
        Validators.required,
        Validators.minLength(3),
        Validators.maxLength(150),
      ]),
      message: new FormControl('', [
        Validators.required,
        Validators.minLength(3),
        Validators.maxLength(1000),
      ]),
    });
  }

  sendMessage() {
    if (this.contactForm.invalid) {
      this.contactForm.markAllAsTouched();
      return;
    }

    let email = this.contactForm.get('email')?.value;
    let name = this.contactForm.get('name')?.value;
    let subject = this.contactForm.get('subject')?.value;
    let message = this.contactForm.get('message')?.value;

    this.contactData = {
      Name: name,
      Email: email,
      Subject: subject,
      Message: message,
    };

    this.sending = true;

    this.contactMessageService.create(this.contactData).subscribe({
      next: () => {
        this.sending = false;
        this.toastService.success('Your message is sent!');
        this.router.navigate(['home']);
      },
      error: (err: any) => {
        this.sending = false;
        this.toastService.error(
          err?.error?.errors?.[0]?.error ||
            'Could not send your message. Please try again.',
        );
      },
    });
  }
}
