import { Component, OnInit } from '@angular/core';
import { HttpHeaders } from '@angular/common/http';
import { AuditLogService } from '../../../Services/audit-log.service';
import { UserService } from '../../../Services/user.service';
import { ToastService } from '../../../Services/toast.service';
import { withMinDuration } from '../../../Utils/loading.util';

@Component({
  selector: 'app-admin-audit-logs',
  templateUrl: './admin-audit-logs.component.html',
  styleUrls: ['./admin-audit-logs.component.scss'],
})
export class AdminAuditLogsComponent implements OnInit {
  headers: any;
  logs: any[] = [];
  dataLoaded: boolean = false;
  loading: boolean = false;
  keyword: string = '';
  userFilter: string = '';
  resultFilter: string = 'all';
  knownUsers: string[] = ['Anonymous'];
  sortColumn: string = 'date';
  sortDirection: 'asc' | 'desc' = 'desc';
  currentPage: number = 1;
  pagesCount: number = 1;
  totalCount: number = 0;
  expandedLogIds: Set<number> = new Set<number>();

  constructor(
    private auditLogService: AuditLogService,
    private userService: UserService,
    private toastService: ToastService,
  ) {}

  ngOnInit(): void {
    const token = localStorage.getItem('token');
    this.headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);
    this.loadUsers();
    this.loadLogs();
  }

  loadUsers(): void {
    this.userService
      .getAll(
        undefined,
        undefined,
        undefined,
        undefined,
        this.headers,
        undefined,
        true,
      )
      .subscribe({
        next: (data: any) => {
          this.addKnownUsers(data.data);

          for (let page = 2; page <= data.pagesCount; page++) {
            this.userService
              .getAll(
                undefined,
                undefined,
                undefined,
                undefined,
                this.headers,
                page,
                true,
              )
              .subscribe({
                next: (more: any) => this.addKnownUsers(more.data),
              });
          }
        },
      });
  }

  private addKnownUsers(users: any[]): void {
    const emails = users
      .map((u: any) => u.email)
      .filter((e: string) => !this.knownUsers.includes(e));
    this.knownUsers = [...this.knownUsers, ...emails].sort((a, b) =>
      a.localeCompare(b),
    );
  }

  loadLogs(): void {
    this.loading = true;
    const isAuthorized =
      this.resultFilter === 'all' ? undefined : this.resultFilter === 'allowed';

    withMinDuration(
      this.auditLogService.search(
        this.keyword || undefined,
        isAuthorized,
        this.userFilter || undefined,
        this.sortColumn,
        this.sortDirection,
        this.currentPage,
        this.headers,
      ),
    ).subscribe({
      next: (data: any) => {
        this.logs = data.data;
        this.pagesCount = Math.max(data.pagesCount, 1);
        this.totalCount = data.count;
        this.dataLoaded = true;
        this.loading = false;
      },
      error: () => {
        this.dataLoaded = true;
        this.loading = false;
        this.toastService.error('Could not load the audit log.');
      },
    });
  }

  search(): void {
    this.currentPage = 1;
    this.loadLogs();
  }

  sortBy(column: string): void {
    if (this.sortColumn === column) {
      this.sortDirection = this.sortDirection === 'asc' ? 'desc' : 'asc';
    } else {
      this.sortColumn = column;
      this.sortDirection = 'asc';
    }

    this.currentPage = 1;
    this.loadLogs();
  }

  previousPage(): void {
    if (this.currentPage > 1) {
      this.currentPage--;
      this.loadLogs();
    }
  }

  nextPage(): void {
    if (this.currentPage < this.pagesCount) {
      this.currentPage++;
      this.loadLogs();
    }
  }

  toggleExpand(id: number): void {
    if (this.expandedLogIds.has(id)) {
      this.expandedLogIds.delete(id);
    } else {
      this.expandedLogIds.add(id);
    }
  }

  shortData(data: string): string {
    if (!data) {
      return '';
    }

    return data.length > 70 ? data.substring(0, 70) + '...' : data;
  }
}
