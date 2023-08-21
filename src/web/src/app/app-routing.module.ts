import { ProjectOverviewComponent } from './modules/project/project-overview/project-overview.component';
import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { MsalGuard } from '@azure/msal-angular';
import { DiagramComponent } from './modules/diagram/diagram/diagram.component';

const routes: Routes = [
  {
    path: '',
    component: ProjectOverviewComponent,
    canActivate: [MsalGuard]
  },
  {
    path: 'diagram/:id',
    component: DiagramComponent,
    canActivate: [MsalGuard]
  }
];

const isIframe = window !== window.parent && !window.opener;

@NgModule({
  imports: [RouterModule.forRoot(routes,
    {
      initialNavigation: !isIframe ? 'enabled' : 'disabled'
    })],
  exports: [RouterModule]
})
export class AppRoutingModule { }
