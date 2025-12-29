// Request to ask a question
export interface AskRequest {
  question: string;
  conversationId?: string; // optional, for multi-turn chat
}

// Response from asking a question
export interface AskResponse {
  conversationId: string;
  answer: string;
  sources: Source[];
  responseTimeMs: number;
  timestamp: string;
}

// Source document info
export interface Source {
  materialId: string;
  title: string;
  pageNumber?: number;
  relevanceScore: number;
}

// Summary of conversations for conversation list page
export interface ConversationSummary {
  conversationId: string;
  title: string;
  lastMessage: string;
  messageCount: number;
  lastUpdated: string;
}

// Detailed messages in a conversation
export interface ConversationDetail {
  conversationId: string;
  messages: ChatMessage[];
}

// Single chat message
export interface ChatMessage {
  role: 'user' | 'assistant'; // user = student, assistant = AI
  content: string;
  timestamp: string;
}
