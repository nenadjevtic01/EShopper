import { Component } from '@angular/core';
import { JwtHelperService } from '@auth0/angular-jwt';
import { currentRole } from './Utils/session.util';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
})
export class AppComponent {
  title = 'EShopper';

  constructor(private jwtHelper: JwtHelperService) {
    currentRole(this.jwtHelper);
  }
}
