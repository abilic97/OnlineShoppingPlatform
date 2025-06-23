import { Component } from '@angular/core';

@Component({
  selector: 'app-navbar',
  templateUrl: './navbar.component.html'
})
export class NavbarComponent {
  loggedIn = false;  // toggle this for demo, replace with real auth logic

  toggleLogin() {
    this.loggedIn = !this.loggedIn;
  }
}
