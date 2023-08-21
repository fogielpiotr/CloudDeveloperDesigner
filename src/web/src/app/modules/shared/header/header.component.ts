import { HttpClient } from '@angular/common/http';
import { Component, OnDestroy, OnInit } from '@angular/core';
import { Router, NavigationEnd } from '@angular/router';
import { MsalService } from '@azure/msal-angular';
import { ConfirmationService, MenuItem } from 'primeng/api';
import { filter, Subject, takeUntil } from 'rxjs';
import { environment } from 'src/environments/environment';
import { UnsubscribeOnDestroy } from '../../core/helpers/unsubscribe-on-destroy';
import { EventService } from '../../core/services/event-service';
import { Project } from '../../project/project.model';

@Component({
  selector: 'app-header',
  templateUrl: './header.component.html',
  styleUrls: ['./header.component.scss']
})
export class HeaderComponent extends UnsubscribeOnDestroy implements OnInit {
  userName: string;
  showBreadcrumb = false;
  selectedProject: Project
  items: MenuItem[];

  home: MenuItem = { icon: 'pi pi-home', routerLink: '/' };

  constructor(
    private router: Router,
    private authService: MsalService,
    private http: HttpClient,
    private confirmationService: ConfirmationService,
    private eventService: EventService) { super() }

  ngOnInit() {
    this.eventService.ProjectSelectedEvent.pipe(takeUntil(this.destroy$)).subscribe(x => {
      this.selectedProject = x;
      this.items = [
        { label: x.name }
      ];
    });
    
    this.getProfile();
    this.router.events.pipe(
      filter(event => event instanceof NavigationEnd)
    )
      .subscribe((event: NavigationEnd) => {
        if (event.url != "/") {
          this.showBreadcrumb = true;
        }
        else {
          this.showBreadcrumb = false;
          this.selectedProject = null;
          this.items = [];
        }
      });
  }

  goHome() {
    this.router.navigate(['']);
  }

  getProfile() {
    this.http.get(environment.graphEndpoint).subscribe((profile: any) => {
      this.userName = profile.displayName;
    });
  }

  logOut() {
    this.confirmationService.confirm({
      key: 'logout',
      message: 'Are you sure that you want to logout?',
      accept: () => {
        this.authService.logoutRedirect({
          postLogoutRedirectUri: environment.redirect
        });
      }
    });
  }
}
