import {Component, OnInit} from '@angular/core';
import {AuthenticationService} from '../../../core/services/authentication.service';
import {RouterService} from '../../../core/services/router.service';

@Component({
  selector: 'app-header',
  templateUrl: './header.component.html',
  styleUrls: ['./header.component.sass']
})
export class HeaderComponent implements OnInit {

  constructor(private authService: AuthenticationService,
              private router: RouterService) {
  }

  ngOnInit() {
  }

  logout() {
    this.authService.logout();
    this.router.navigate('login');
  }
}
