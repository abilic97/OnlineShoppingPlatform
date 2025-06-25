import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { UserService } from '../services/users.service';

@Component({
  selector: 'app-navbar',
  templateUrl: './navbar.component.html'
})
export class NavbarComponent {

  constructor(private userService: UserService, private router: Router) { }

  get loggedIn(): boolean {
    return !!this.userService.getToken();
  }

  toggleLogin() {
    if (this.loggedIn) {
      this.userService.removeToken();
      this.router.navigate(['/login']);
    } else {
      this.router.navigate(['/login']);
    }
  }
}