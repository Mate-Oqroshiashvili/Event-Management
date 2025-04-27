import { Component } from '@angular/core';
import {
  CommentDto,
  CommentService,
} from '../../../services/comment/comment.service';
import { ActivatedRoute, RouterModule } from '@angular/router';

@Component({
  selector: 'app-comments-user-added',
  imports: [RouterModule],
  templateUrl: './comments-user-added.component.html',
  styleUrl: './comments-user-added.component.css',
})
export class CommentsUserAddedComponent {
  userId: number = 0;
  comments: CommentDto[] = [];

  constructor(
    private commentService: CommentService,
    private route: ActivatedRoute
  ) {}

  ngOnInit(): void {
    this.route.parent?.paramMap.subscribe((data) => {
      this.userId = +data.get('userId')!;
      this.getCommentsByUserId();
    });
  }

  getCommentsByUserId() {
    this.commentService.getCommentsByUserId(this.userId).subscribe({
      next: (data: any) => {
        this.comments = data.comments;
      },
      error: (err) => {
        console.error(err);
      },
      complete: () => {
        console.log('Commets fetched successfully!');
      },
    });
  }
}
