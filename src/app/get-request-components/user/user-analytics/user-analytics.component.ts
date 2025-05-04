import { Component, OnInit } from '@angular/core';
import {
  UserAnalyticsDto,
  UserService,
} from '../../../services/user/user.service';
import { CommonModule } from '@angular/common';
import { ActivatedRoute } from '@angular/router';
import { UserSocketService } from '../../../services/web sockets/user-socket.service';
import { AnalyticsService } from '../../../services/analytics/analytics.service';

@Component({
  selector: 'app-user-analytics',
  imports: [CommonModule],
  templateUrl: './user-analytics.component.html',
  styleUrl: './user-analytics.component.css',
})
export class UserAnalyticsComponent implements OnInit {
  analytics!: UserAnalyticsDto;
  userId: number = 0;
  balance: number = 0;

  balanceSpendingGradient = '';
  promoCodeGradient = '';

  stats: { label: string; value: string | number }[] = [];

  constructor(
    private analyticsService: AnalyticsService,
    private userSocket: UserSocketService,
    private route: ActivatedRoute
  ) {}

  ngOnInit(): void {
    this.route.parent?.paramMap.subscribe((data) => {
      const userIdParam = data.get('userId');
      if (userIdParam) {
        this.userId = +userIdParam;
        this.analyticsService
          .getUserAnalytics(this.userId)
          .subscribe((data) => {
            this.balance = data.totalBalance;
            this.analytics = data;
            this.setGradients();
            this.setStats();
          });
      } else {
        console.error('User ID not found in route parameters');
      }
    });

    this.userSocket.startConnection();

    this.userSocket.balance$.subscribe((data) => {
      this.balance = data!;
    });
  }

  setGradients(): void {
    const total = this.analytics.totalBalance + this.analytics.totalSpent;
    const spentPercent = (this.analytics.totalSpent / total) * 100;
    const balancePercent = (this.analytics.totalBalance / total) * 100;

    this.balanceSpendingGradient = `conic-gradient(
        #28a745 0% ${balancePercent}%,
        #dc3545 ${balancePercent}% 100%
      )`;
  }

  setStats(): void {
    const a = this.analytics;
    this.stats = [
      { label: 'Total Purchases', value: a.totalPurchases },
      { label: 'Tickets Bought', value: a.totalTicketsBought },
      { label: 'Events Participated In', value: a.eventsParticipatedIn },
      { label: 'Events as Artist/Speaker', value: a.eventsAsArtistOrSpeaker },
      { label: 'Comments Posted', value: a.totalComments },
      { label: 'Reviews Given', value: a.totalReviews },
      { label: 'Promo Codes Used', value: a.usedPromoCodesCount },
    ];
  }
}
