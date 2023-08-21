import { CardModule } from 'primeng/card';
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { ReactiveFormsModule } from '@angular/forms';
import { ButtonModule } from 'primeng/button';
import { MessagesModule } from 'primeng/messages';
import { DialogModule } from 'primeng/dialog';
import { ToastModule } from 'primeng/toast';
import { AccordionModule } from 'primeng/accordion';
import { AutoCompleteModule } from 'primeng/autocomplete';
import { AvatarModule } from 'primeng/avatar';
import { ChipsModule } from 'primeng/chips';
import { DropdownModule } from 'primeng/dropdown';
import { InputTextModule } from 'primeng/inputtext';
import { InputTextareaModule } from 'primeng/inputtextarea';
import { MenubarModule } from 'primeng/menubar';
import { MessageModule } from 'primeng/message';
import { MultiSelectModule } from 'primeng/multiselect';
import { RippleModule } from 'primeng/ripple';
import { SpeedDialModule } from 'primeng/speeddial';
import { SplitButtonModule } from 'primeng/splitbutton';
import { TableModule } from 'primeng/table';
import { TabViewModule } from 'primeng/tabview';
import { ToolbarModule } from 'primeng/toolbar';
import { NgxSpinnerModule } from 'ngx-spinner';
import { ScrollPanelModule } from 'primeng/scrollpanel';
import { DividerModule } from 'primeng/divider';
import { MatSidenavModule } from '@angular/material/sidenav';
import { TooltipModule } from 'primeng/tooltip';
import { HeaderComponent } from './header/header.component';
import { CheckboxModule } from 'primeng/checkbox';
import { MatToolbarModule } from '@angular/material/toolbar';
import { SelectButtonModule } from 'primeng/selectbutton';
import { BreadcrumbModule } from 'primeng/breadcrumb';
import { FormControlErrorComponent } from './form-control-error/form-control-error.component';
import { TimelineModule } from 'primeng/timeline';

@NgModule({
  declarations: [
    HeaderComponent,
    FormControlErrorComponent
  ],
  imports: [
    CommonModule,
    ButtonModule,
    BreadcrumbModule,
    ConfirmDialogModule,
  ],
  exports: [
    CommonModule,
    CheckboxModule,
    SelectButtonModule,
    ConfirmDialogModule,
    MessagesModule,
    ReactiveFormsModule,
    ButtonModule,
    DialogModule,
    ToastModule,
    InputTextModule,
    ReactiveFormsModule,
    ToolbarModule,
    DropdownModule,
    MessageModule,
    MessagesModule,
    ToastModule,
    RippleModule,
    MultiSelectModule,
    AutoCompleteModule,
    DialogModule,
    TabViewModule,
    AccordionModule,
    AvatarModule,
    ChipsModule,
    SplitButtonModule,
    TableModule,
    InputTextareaModule,
    SpeedDialModule,
    MenubarModule,
    NgxSpinnerModule,
    ScrollPanelModule,
    CardModule,
    DividerModule,
    TooltipModule,
    HeaderComponent,
    TimelineModule,
    MatToolbarModule,
    MatSidenavModule,
    FormControlErrorComponent
  ]
})
export class SharedModule { }
