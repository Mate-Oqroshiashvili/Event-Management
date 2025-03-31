import { Routes } from '@angular/router';
import { HomeComponent } from './home/home.component';
import { NotFoundComponent } from './not-found/not-found.component';
import { AdminPanelComponent } from './admin-panel/admin-panel.component';
import { AvailablePromoCodesComponent } from './available-promo-codes/available-promo-codes.component';
import { EventsComponent } from './events/events.component';
import { EventPageComponent } from './event-page/event-page.component';
import { LocationsComponent } from './locations/locations.component';
import { LocationPageComponent } from './location-page/location-page.component';
import { RegisterComponent } from './register/register.component';
import { LoginComponent } from './login/login.component';
import { OrganizersComponent } from './organizers/organizers.component';
import { OrganizerPageComponent } from './organizer-page/organizer-page.component';
import { OrganizerPanelComponent } from './organizer-panel/organizer-panel.component';
import { ProfileComponent } from './profile/profile.component';
import { UserInfoComponent } from './user-info/user-info.component';
import { UserAnalyticsComponent } from './user-analytics/user-analytics.component';
import { SearchResultComponent } from './search-result/search-result.component';
import { authGuard } from './services/guards/auth.guard';

export const routes: Routes = [
  { path: '', component: HomeComponent },
  { path: 'register', component: RegisterComponent },
  { path: 'login', component: LoginComponent },
  {
    path: 'search-result/:searchTerm',
    component: SearchResultComponent,
    canActivate: [authGuard],
  },
  {
    path: 'profile',
    component: ProfileComponent,
    canActivate: [authGuard],
    canActivateChild: [authGuard],
    children: [
      { path: 'user-info/:userId', component: UserInfoComponent },
      {
        path: 'user-participation-history/:userId',
        component: UserInfoComponent,
      },
      { path: 'user-analytics/:userId', component: UserAnalyticsComponent },
    ],
  },
  {
    path: 'organizer-panel',
    component: OrganizerPanelComponent,
    canActivate: [authGuard],
  },
  {
    path: 'admin-panel',
    component: AdminPanelComponent,
    canActivate: [authGuard],
  },
  {
    path: 'available-promo-codes',
    component: AvailablePromoCodesComponent,
    canActivate: [authGuard],
  },
  { path: 'events', component: EventsComponent, canActivate: [authGuard] },
  {
    path: 'events/event/:eventId',
    component: EventPageComponent,
    canActivate: [authGuard],
  },
  {
    path: 'locations',
    component: LocationsComponent,
    canActivate: [authGuard],
  },
  {
    path: 'locations/location/:locationId',
    component: LocationPageComponent,
    canActivate: [authGuard],
  },
  {
    path: 'organizers',
    component: OrganizersComponent,
    canActivate: [authGuard],
  },
  {
    path: 'organizers/organizer/:organizerId',
    component: OrganizerPageComponent,
    canActivate: [authGuard],
  },
  { path: '**', component: NotFoundComponent },
];
