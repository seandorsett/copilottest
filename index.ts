function add(x: number, y: number): number {
    return x + y;
}

function subtract(x: number, y: number): number {
    return x - y;
}

// User type enum for different user privileges
type UserType = 'member' | 'administrator';

// User interface for the user management system
interface User {
    id: string;
    name: string;
    userType: UserType;
}

// Book interface for the favorites system
interface Book {
    id: string;
    title: string;
    author: string;
    isbn?: string;
    publishedYear?: number;
}

// Favorites Manager class to handle book favorites functionality
class FavoritesManager {
    private favorites: Book[] = [];

    /**
     * Add a book to the favorites list
     */
    addToFavorites(book: Book): boolean {
        // Check if book is already in favorites
        if (this.favorites.some(fav => fav.id === book.id)) {
            return false; // Already in favorites
        }
        
        this.favorites.push(book);
        return true;
    }

    /**
     * Remove a book from the favorites list
     * @param bookId - The ID of the book to remove
     * @param showConfirmation - Whether to show confirmation dialog (default: true)
     * @returns Promise<boolean> - true if removed, false if cancelled or not found
     */
    async removeFromFavorites(bookId: string, showConfirmation: boolean = true): Promise<boolean> {
        const bookIndex = this.favorites.findIndex(book => book.id === bookId);
        
        if (bookIndex === -1) {
            return false; // Book not found in favorites
        }

        const book = this.favorites[bookIndex];

        if (showConfirmation) {
            const confirmed = await this.showConfirmationDialog(book);
            if (!confirmed) {
                return false; // User cancelled
            }
        }

        // Remove the book from favorites
        this.favorites.splice(bookIndex, 1);
        return true;
    }

    /**
     * Get all favorite books
     */
    getFavorites(): Book[] {
        return [...this.favorites]; // Return a copy to prevent external modification
    }

    /**
     * Check if a book is in favorites
     */
    isFavorite(bookId: string): boolean {
        return this.favorites.some(book => book.id === bookId);
    }

    /**
     * Get the number of favorite books
     */
    getFavoritesCount(): number {
        return this.favorites.length;
    }

    /**
     * Clear all favorites
     */
    clearFavorites(): void {
        this.favorites = [];
    }

    /**
     * Show confirmation dialog for removing a book
     * In a real app, this would show a proper modal/dialog
     */
    private async showConfirmationDialog(book: Book): Promise<boolean> {
        return new Promise((resolve) => {
            const message = `Are you sure you want to remove "${book.title}" by ${book.author} from your favorites?`;
            
            // In a browser environment, use confirm dialog
            if (typeof window !== 'undefined' && window.confirm) {
                resolve(window.confirm(message));
            } else {
                // In Node.js environment, simulate confirmation (default to true for testing)
                console.log(`Confirmation: ${message}`);
                resolve(true);
            }
        });
    }
}

export { add, subtract, Book, User, UserType, FavoritesManager };
export default { add, subtract, FavoritesManager };

// IGNORE
// Add GitHub Copilot instructions and VSCode settings
// to the root of the project to enable AI-assisted coding.
// This will help you write code faster and with fewer errors.
// Instructions:
// 1. Install the GitHub Copilot extension in VSCode.
// 2. Sign in with your GitHub account.
// 3. Enable Copilot for this workspace.
// 4. Start coding and let Copilot assist you!
// VSCode Settings (settings.json):
// {
//     "github.copilot.enable": true,
//     "github.copilot.suggestionDelay": 100,
//     "github.copilot.inlineSuggest.enable": true
