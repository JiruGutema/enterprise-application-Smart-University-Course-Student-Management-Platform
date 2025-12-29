import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import {
  AskRequest,
  AskResponse,
  ConversationSummary,
  ConversationDetail,
} from '../models/assistant.types';

@Injectable({
  providedIn: 'root',
})
export class AssistantService {
  private base = '/api/ai'; // Base URL for AI backend

  constructor(private http: HttpClient) {}

  // Ask a course-specific question
  ask(courseId: string, body: AskRequest): Observable<AskResponse> {
    return this.http.post<AskResponse>(
      `${this.base}/courses/${courseId}/ask`,
      body
    );
  }

  // Summarize course material
  summarize(courseId: string, body: any): Observable<any> {
    return this.http.post(`${this.base}/courses/${courseId}/summarize`, body);
  }

  // Get list of conversations for a course
  getConversations(
    courseId: string
  ): Observable<{ data: ConversationSummary[] }> {
    return this.http.get<{ data: ConversationSummary[] }>(
      `${this.base}/courses/${courseId}/conversations`
    );
  }

  // Get details of a single conversation
  getConversationDetail(
    conversationId: string
  ): Observable<ConversationDetail> {
    return this.http.get<ConversationDetail>(
      `${this.base}/conversations/${conversationId}`
    );
  }

  // Get AI embedding status (instructor/admin)
  getEmbeddingStatus(courseId: string): Observable<any> {
    return this.http.get(`${this.base}/courses/${courseId}/embedding-status`);
  }

  // Trigger re-embedding (instructor/admin)
  reEmbed(courseId: string): Observable<any> {
    return this.http.post(`${this.base}/courses/${courseId}/re-embed`, {});
  }

  // Get AI usage analytics
  getUsage(
    courseId: string,
    period: 'week' | 'month' | 'all'
  ): Observable<any> {
    return this.http.get(
      `${this.base}/courses/${courseId}/usage?period=${period}`
    );
  }
}
