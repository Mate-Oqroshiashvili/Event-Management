import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import {
  AdminAnalyticsDto,
  UserService,
} from '../../services/user/user.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-admin-panel',
  imports: [CommonModule],
  templateUrl: './admin-panel.component.html',
  styleUrl: './admin-panel.component.css',
})
export class AdminPanelComponent implements OnInit {
  analytics!: AdminAnalyticsDto;

  userRoleGradient = '';
  eventStatusGradient = '';

  constructor(private analyticsService: UserService, private router: Router) {}

  ngOnInit(): void {
    this.analyticsService.getAdminAnalytics().subscribe((data) => {
      this.analytics = data;
      this.setGradients();
    });
  }

  setGradients(): void {
    const u = this.analytics;
    const totalUsers = u.totalUsers + u.totalOrganizers + u.totalParticipants;
    const totalEvents =
      u.draftedEvents + u.publishedEvents + u.completedEvents + u.deletedEvents;

    // User Roles Gradient
    const u1 = (u.totalUsers / totalUsers) * 100;
    const u2 = u1 + (u.totalOrganizers / totalUsers) * 100;
    const u3 = u2 + (u.totalParticipants / totalUsers) * 100;

    this.userRoleGradient = `conic-gradient(
        #007bff 0% ${u1}%,
        #ffc107 ${u1}% ${u2}%,
        #28a745 ${u2}% ${u3}%,
        #e0e0e0 ${u3}% 100%
      )`;

    // Event Status Gradient
    const e1 = (u.draftedEvents / totalEvents) * 100;
    const e2 = e1 + (u.publishedEvents / totalEvents) * 100;
    const e3 = e2 + (u.completedEvents / totalEvents) * 100;
    const e4 = e3 + (u.deletedEvents / totalEvents) * 100;

    this.eventStatusGradient = `conic-gradient(
        #6c757d 0% ${e1}%,
        #17a2b8 ${e1}% ${e2}%,
        #28a745 ${e2}% ${e3}%,
        #dc3545 ${e3}% ${e4}%,
        #e0e0e0 ${e4}% 100%
      )`;
  }

  onAddLocation(): void {
    this.router.navigate(['/admin-panel/add-location']);
  }
}
