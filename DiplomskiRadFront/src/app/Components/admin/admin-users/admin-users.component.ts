import { Component, OnInit } from '@angular/core';
import { HttpHeaders } from '@angular/common/http';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { UserService } from '../../../Services/user.service';
import { ToastService } from '../../../Services/toast.service';
import { ConfirmService } from '../../../Services/confirm.service';
import { withMinDuration } from '../../../Utils/loading.util';

@Component({
  selector: 'app-admin-users',
  templateUrl: './admin-users.component.html',
  styleUrls: ['./admin-users.component.scss'],
})
export class AdminUsersComponent implements OnInit {
  headers: any;
  users: any[] = [];
  dataLoaded: boolean = false;
  loading: boolean = false;
  keyword: string = '';
  includeBlocked: boolean = false;
  errorMessage: string = '';

  showCreateForm: boolean = false;
  createForm!: FormGroup;
  creating: boolean = false;
  lastInvite: { email: string; resetLink: string; emailSent: boolean } | null =
    null;

  editingUserId: number | null = null;
  editForm!: FormGroup;
  saving: boolean = false;

  constructor(
    private userService: UserService,
    private toastService: ToastService,
    private confirmService: ConfirmService,
  ) {}

  ngOnInit(): void {
    const token = localStorage.getItem('token');
    this.headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);

    this.createForm = new FormGroup<any>({
      firstName: new FormControl('', [Validators.required]),
      lastName: new FormControl('', [Validators.required]),
      email: new FormControl('', [
        Validators.required,
        Validators.email,
        Validators.maxLength(100),
      ]),
      userName: new FormControl('', [Validators.required]),
      address: new FormControl(''),
      city: new FormControl(''),
      postalCode: new FormControl(''),
      country: new FormControl(''),
      role: new FormControl(2),
    });

    this.editForm = new FormGroup<any>({
      userId: new FormControl(0),
      firstName: new FormControl('', [Validators.required]),
      lastName: new FormControl('', [Validators.required]),
      email: new FormControl('', [
        Validators.required,
        Validators.email,
        Validators.maxLength(100),
      ]),
      userName: new FormControl('', [Validators.required]),
      address: new FormControl(''),
      city: new FormControl(''),
      postalCode: new FormControl(''),
      country: new FormControl(''),
    });

    this.loadUsers();
  }

  loadUsers(): void {
    this.loading = true;

    withMinDuration(
      this.userService.getAll(
        this.keyword || undefined,
        undefined,
        undefined,
        undefined,
        this.headers,
        undefined,
        this.includeBlocked,
      ),
    ).subscribe({
      next: (data: any) => {
        this.users = data.data;
        this.dataLoaded = true;
        this.loading = false;
      },
      error: () => {
        this.dataLoaded = true;
        this.loading = false;
        this.errorMessage = 'Could not load users.';
      },
    });
  }

  search(): void {
    this.loadUsers();
  }

  toggleCreateForm(): void {
    this.showCreateForm = !this.showCreateForm;

    if (this.showCreateForm) {
      this.editingUserId = null;
      this.lastInvite = null;
    }
  }

  createUser(): void {
    if (this.createForm.invalid) {
      this.createForm.markAllAsTouched();
      return;
    }

    const dataToSend = {
      FirstName: this.createForm.get('firstName')?.value,
      LastName: this.createForm.get('lastName')?.value,
      Email: this.createForm.get('email')?.value,
      UserName: this.createForm.get('userName')?.value,
      Address: this.createForm.get('address')?.value,
      City: this.createForm.get('city')?.value,
      PostalCode: this.createForm.get('postalCode')?.value,
      Country: this.createForm.get('country')?.value,
      Role: Number(this.createForm.get('role')?.value),
    };

    this.creating = true;

    this.userService.adminCreate(dataToSend, this.headers).subscribe({
      next: (result: any) => {
        this.creating = false;
        this.createForm.reset({ role: 2 });
        this.lastInvite = {
          email: result.email,
          resetLink: result.resetLink,
          emailSent: result.emailSent,
        };
        const what = dataToSend.Role === 1 ? 'Administrator' : 'User';

        this.toastService.success(
          result.emailSent
            ? what + ' created. An email with a set-password link was sent.'
            : what +
                ' created. The invite email could not be sent - share the link below manually.',
        );

        this.loadUsers();
      },
      error: (err: any) => {
        this.creating = false;

        this.toastService.error(
          err?.error?.errors?.[0]?.error ||
            'Could not create user. Email might already be in use.',
        );
      },
    });
  }

  startEdit(user: any): void {
    this.showCreateForm = false;
    this.lastInvite = null;
    this.editingUserId = user.id;

    this.editForm.setValue({
      userId: user.id,
      firstName: user.firstName || '',
      lastName: user.lastName || '',
      email: user.email || '',
      userName: user.username || '',
      address: user.info?.address || '',
      city: user.info?.city || '',
      postalCode: user.info?.postalCode || '',
      country: user.info?.country || '',
    });
  }

  cancelEdit(): void {
    this.editingUserId = null;
  }

  initials(user: any): string {
    return [user.firstName, user.lastName]
      .filter((x: string) => !!x)
      .map((x: string) => x[0])
      .join('')
      .toUpperCase();
  }

  saveEdit(): void {
    if (this.editForm.invalid) {
      this.editForm.markAllAsTouched();
      return;
    }

    const dataToSend = {
      UserId: this.editForm.get('userId')?.value,
      FirstName: this.editForm.get('firstName')?.value,
      LastName: this.editForm.get('lastName')?.value,
      Email: this.editForm.get('email')?.value,
      UserName: this.editForm.get('userName')?.value,
      Address: this.editForm.get('address')?.value,
      City: this.editForm.get('city')?.value,
      PostalCode: this.editForm.get('postalCode')?.value,
      Country: this.editForm.get('country')?.value,
    };

    this.saving = true;

    this.userService.update(dataToSend, this.headers).subscribe({
      next: () => {
        this.saving = false;
        this.editingUserId = null;
        this.toastService.success('User updated successfully.');
        this.loadUsers();
      },
      error: (err: any) => {
        this.saving = false;
        this.toastService.error(
          err?.error?.errors?.[0]?.error || 'Could not update user.',
        );
      },
    });
  }

  async toggleBlockUser(
    id: number,
    name: string,
    isBanned: boolean,
  ): Promise<void> {
    const message = isBanned
      ? `Unblock ${name}? They will be able to log in again.`
      : `Block ${name}? They will no longer be able to log in.`;

    const confirmed = await this.confirmService.confirm(
      message,
      isBanned ? 'Unblock user' : 'Block user',
      isBanned ? 'Unblock' : 'Block',
      !isBanned,
    );

    if (!confirmed) {
      return;
    }

    this.userService.delete(id, this.headers).subscribe({
      next: () => {
        this.toastService.success(
          isBanned ? 'User unblocked.' : 'User blocked.',
        );
        this.loadUsers();
      },
      error: (err: any) => {
        this.toastService.error(
          err?.error?.errors?.[0]?.error || 'Could not update user.',
        );
      },
    });
  }
}
